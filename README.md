# Desafio de Código

Nesse desafio foi solicitado o desenvolvimento de um aplicativo que se comunicasse com uma api externa,
onde é possível criar usuário, solicitar autorização através de um token e com esse token solicitar desafios via método GET,
que devem ser solucionados e retornados para a api através do método POST.

Descrição do desafio:


<h2 id="intro">1. Intro</h2>
In order to improve our research and computing power, we decided to distribute some computing operations.<br>
Your job is to follow the documentation below and create a <strong>C# .NET</strong> program which communicates with our APIs and help us complete DNA operations.</p>
<h2 id="enroll">2. Enroll</h2>
<p>You will need to handle top secret information, so the first step is to create a user in the system and request an AccessToken for communication with the authenticated APIs.</p>
<h3 id="create-user">2.1. Create user</h3>
<pre class=" language-ts"><code class="prism  language-ts"><span class="token string">'[POST] /api/users/create'</span>
Request body
<span class="token punctuation">{</span>
  <span class="token comment">// Allowed a-z, A-Z and 0-9 chars only</span>
  <span class="token comment">// Min size 4 chars and max size 32 chars</span>
  username<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span>

  <span class="token comment">// Your email address so we are able to contact you</span>
  email<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span>

  <span class="token comment">// Min size 8 chars</span>
  password<span class="token punctuation">:</span> <span class="token keyword">string</span> <span class="token comment">//</span>
<span class="token punctuation">}</span>

Response
<span class="token punctuation">{</span>
  code<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span> <span class="token comment">// ['Success', 'Error']</span>
  message<span class="token operator">?</span><span class="token punctuation">:</span> <span class="token keyword">string</span>
<span class="token punctuation">}</span>
</code></pre>
<h3 id="request-access-token">2.2 Request access token</h3>
<pre class=" language-ts"><code class="prism  language-ts"><span class="token string">'[POST] /api/users/login'</span>
Request body
<span class="token punctuation">{</span>
  username<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span>
  password<span class="token punctuation">:</span> <span class="token keyword">string</span>
<span class="token punctuation">}</span>

Response
<span class="token punctuation">{</span>
  accessToken<span class="token operator">?</span><span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span>
  code<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span><span class="token comment">// ['Success', 'Error']</span>
  message<span class="token operator">?</span><span class="token punctuation">:</span> <span class="token keyword">string</span>
<span class="token punctuation">}</span>
</code></pre>
<p>If everything is OK, you will receive a <code>Success</code>response code and an <code>AccessToken</code> string. The access token is meant to be used in the <code>Authorization</code> Header parameter as an OAuth bearer token scheme.<br>
The access token is valid for 2 minutes, if expired the authenticated APIs will return an <code>Unauthorized</code> response code with message: “Bad token: token is expired”, you will need to request a new one.</p>
<h2 id="quick-bio-review">3. Quick Bio review</h2>
<p>The <strong>DNA</strong> is a structure composed of a double-stranded helix. The two strands are connected by hydrogen bonds and each end of the bond has a nucleobase. The DNA possible nucleobases are <strong>A</strong>denine, <strong>C</strong>ytosine, <strong>G</strong>uanine and <strong>T</strong>hymine, in a way that <strong>A</strong> always pairs with <strong>T</strong> and <strong>C</strong> always pairs with <strong>G</strong></p>
<p><img src="/img/dna.png" alt="" width="200"></p>
<h3 id="dna-strands">3.1. DNA strands</h3>
<p>For this <strong>fictional experiment</strong> we consider the main strand as the template strand, <em>i.e.</em> the strand which is used to transcript RNA. Also for this experiment, the main strand segments presented will always begin with the nucleobases sequence: <strong>C-A-T</strong>, so it is a simple task to differentiate the template strand from the complementary strand or to compute one from the other just inverting the nucleobases pairs.</p>
<p><img src="/img/strands.png" alt=""></p>
<h2 id="dna-encoding">4. DNA encoding</h2>
<p>You shall expect DNA strand segments encoded in both binary and string formats:</p>
<h3 id="binary-format">4.1. Binary format</h3>
<p>Is the short format used for better data transmission and storage performance. In this format the nucleobases are encoded in 2 bits arrays:</p>
<pre><code>A: 0b00      C: 0b01
T: 0b11      G: 0b10
</code></pre>
<h3 id="string-format">4.2. String format</h3>
<p>Used for better human understanding. In this format the nucleobases are encoded as its char: <code>"CATCGTCAGGACTCAGTCCATCTTAACTACTAAACTC..."</code></p>
<h3 id="encoding-example">4.3. Encoding example</h3>
<p>Encoding from String to Binary format example:</p>
<pre><code>String:   "CATCGTCAGGAC"
Bits:     0b010011011011010010100001
Byte[]:   [0x4D, 0xB4, 0xA1] // notice the bits to byte conversion is Big-Endian
Base64:   "TbSh"
</code></pre>
<h2 id="operations">5. Operations</h2>
<h3 id="request-a-job">5.1. Request a job</h3>
<pre class=" language-ts"><code class="prism  language-ts"><span class="token string">'[GET] /api/dna/jobs'</span>
Header
  Authorization = 'Bearer <AccessToken>' // <AccessToken> aquired on 2.2

Response
{
  job?: {
    // Job id
    id: string,
    
    // Operation types ['DecodeStrand', 'EncodeStrand', 'CheckGene']
    type: string,
    
    // Strand in String format. Non-null when operation type 'EncodeStrand'
    strand?: string,

    // Strand in the Binary format Base64 encoded. Non-null when operation types 'DecodeStrand' and 'CheckGene'
    strandEncoded?: string,

    // A gene segment in the Binary format Base64 encoded. Non-null when operation type 'CheckGene'
    geneEncoded?: string
  },
  code: string, // ['Success', 'Error', 'Unauthorized']
  message?: string
}
</code></pre>
<p>If everything is OK you will receive a job object with the job <code>id</code>, the operation <code>type</code> and operation parameters which you are able to solve as follows.</p>
<h3 id="decode-strand-operation">5.2. Decode strand operation</h3>
<p>If you receive a <code>'DecodeStrand'</code> operation, the job is to take the <code>strandEncoded</code> parameter, which is a Base64 string of the strand in Binary format, and decode it to the String format according to session <strong>4</strong>.<br>
For this operation you shall send the response to:</p>
<pre class=" language-ts"><code class="prism  language-ts"><span class="token string">'[POST] /api/dna/jobs/{id}/decode'</span>
URL Parameters
  id <span class="token comment">// The Job id</span>

Header
  Authorization <span class="token operator">=</span> <span class="token string">'Bearer &lt;AccessToken&gt;'</span> <span class="token comment">// &lt;AccessToken&gt; aquired on 2.2</span>

Request body
<span class="token punctuation">{</span>
  <span class="token comment">// Decoded strand in String format</span>
  strand<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span>
<span class="token punctuation">}</span>

Response
<span class="token punctuation">{</span>
  code<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span> <span class="token comment">// ['Success', 'Error', 'Fail', 'Unauthorized']</span>
  message<span class="token operator">?</span><span class="token punctuation">:</span> <span class="token keyword">string</span>
<span class="token punctuation">}</span>
</code></pre>
<h3 id="encode-strand-operation">5.3. Encode strand operation</h3>
<p>If you receive a <code>'EncodeStrand'</code> operation, the job is to take the <code>strand</code> parameter, which is the strand in String format, and encode it to the Binary format according to session <strong>4</strong>.<br>
For this operation you shall send the response to:</p>
<pre class=" language-ts"><code class="prism  language-ts"><span class="token string">'[POST] /api/dna/jobs/{id}/encode'</span>
URL Parameters
  id <span class="token comment">// The Job id</span>

Header
  Authorization <span class="token operator">=</span> <span class="token string">'Bearer &lt;AccessToken&gt;'</span> <span class="token comment">// &lt;AccessToken&gt; aquired on 2.2</span>

Request body
<span class="token punctuation">{</span>
  <span class="token comment">// Encoded strand in Binary format Base64</span>
  strandEncoded<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span>
<span class="token punctuation">}</span>

Response
<span class="token punctuation">{</span>
  code<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span> <span class="token comment">// ['Success', 'Error', 'Fail', 'Unauthorized']</span>
  message<span class="token operator">?</span><span class="token punctuation">:</span> <span class="token keyword">string</span>
<span class="token punctuation">}</span>
</code></pre>
<h3 id="check-gene-operation">5.4. Check gene operation</h3>
<p>If you receive a <code>'CheckGene'</code> operation, the job is to tell whether or not a particular gene is activated in the retrieved DNA strand. Both gene and DNA strands are retrieved in Binary formats.<br>
For this experiment, a gene is considered activated <strong>if more than 50%</strong> of its content is present in the DNA template strand. Ex:<br>
Gene:<br>
TACCGCTTCA<mark>TAAACCGCTAGACTGCATGATCG</mark>GGT</p>
<p>DNA template strand:<br>
CATCTCAGTCCTACTAAACTCGCGAAGCTCATACTAGCTAC<mark>TAAACCGCTAGACTGCATGATCG</mark>CATAGCTAGCTACGCT</p>
<p>In the example above more than 50% of the gene (~63% of the gene) is present on the template strand, so in this case the gene is considered <strong>activated</strong>.</p>
<p><strong>REMARK:</strong> Please notice that the gene comparison shall be applied over the DNA <strong>template strand</strong>. So you need to check according to session <strong>3.1</strong> if the retrieved strand is the template or the complementary one and compute each other if necessary before searching for the gene segments presence.</p>
<p>For this operation you shall send the response to:</p>
<pre class=" language-ts"><code class="prism  language-ts"><span class="token string">'[POST] /api/dna/jobs/{id}/gene'</span>
URL Parameters
  id <span class="token comment">// The Job id</span>

Header
  Authorization <span class="token operator">=</span> <span class="token string">'Bearer &lt;AccessToken&gt;'</span> <span class="token comment">// &lt;AccessToken&gt; aquired on 2.2</span>

Request body
<span class="token punctuation">{</span>
  <span class="token comment">// Whether or not the gene is activated in the template strand</span>
  isActivated<span class="token punctuation">:</span> <span class="token keyword">boolean</span><span class="token punctuation">,</span>
<span class="token punctuation">}</span>

Response
<span class="token punctuation">{</span>
  code<span class="token punctuation">:</span> <span class="token keyword">string</span><span class="token punctuation">,</span> <span class="token comment">// ['Success', 'Error', 'Fail', 'Unauthorized']</span>
  message<span class="token operator">?</span><span class="token punctuation">:</span> <span class="token keyword">string</span>
<span class="token punctuation">}</span>
</code></pre>
